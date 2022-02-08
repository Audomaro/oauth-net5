import { HttpClient } from '@angular/common/http'
import { Injectable } from '@angular/core'
import { Router } from '@angular/router'
import { BehaviorSubject, map, Observable } from 'rxjs'
import { environment } from 'src/environments/environment'

import { User } from '../models/user.model'

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private clearUser: User;
  private userSubject$: BehaviorSubject<User>;
  public user: Observable<User>;

  constructor(
    private router: Router,
    private http: HttpClient
  ) {
    this.clearUser = {
      id: 0,
      username: '',
      branch: '',
      displayName: '',
      email: ''
    };

    this.userSubject$ = new BehaviorSubject<User>(this.clearUser);
    this.user = this.userSubject$.asObservable();
  }

  public get userValue(): User {
    return this.userSubject$.value;
  }

  public login(username: string, password: string): Observable<User> {
    return this.http.post<any>(`${environment.apiUrl}/authentication/authenticate`, { username, password }, { withCredentials: true })
      .pipe(
        map((user: User) => {
          this.userSubject$.next(user);
          this.startRefreshTokenTimer();
          return user;
        })
      );
  }

  public logout(): void {


    this.http.post<any>(`${environment.apiUrl}/authentication/revoke-token`, {}, { withCredentials: true })
      .subscribe(() => {
        this.closeSession();
      });
  }

  private closeSession() {
    this.stopRefreshTokenTimer()
    this.userSubject$.next(this.clearUser)
    this.router.navigate(['login'])
  }

  public refreshToken(): Observable<User> {
    return this.http.post<any>(`${environment.apiUrl}/authentication/refresh-token`, {}, { withCredentials: true })
      .pipe(
        map((user: User) => {
          this.userSubject$.next(user);
          this.startRefreshTokenTimer();
          return user;
        })
      );
  }

  public isAuthenticated(): boolean {
    return this.userSubject$.value.jwtToken !== undefined;
  }
  private refreshTokenTimeout: any;

  private startRefreshTokenTimer(): void {

    const tempUser = this.userValue;

    if (tempUser.jwtToken) {
      // parse json object from base64 encoded jwt token
      const jwtToken = JSON.parse(atob(tempUser.jwtToken.split('.')[1]));

      // set a timeout to refresh the token a minute before it expires
      const expires = new Date(jwtToken.exp * 1000);
      const timeout = expires.getTime() - Date.now() - (1 * 1000);
      this.refreshTokenTimeout = setTimeout(() => this.refreshToken().subscribe(), timeout);
    }
  }

  private stopRefreshTokenTimer() {
    clearTimeout(this.refreshTokenTimeout);
  }
}
