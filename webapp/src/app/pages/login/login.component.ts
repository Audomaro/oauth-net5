import { Component, OnInit, ViewChild } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { NotifierService } from 'angular-notifier'
import { DxFormComponent } from 'devextreme-angular'
import { NgxSpinnerService } from 'ngx-spinner'
import { first } from 'rxjs'
import { AuthenticateRequest } from 'src/app/models'
import { LanguageService } from 'src/app/services'

import { AuthenticationService } from './../../services/authentication.service'

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  @ViewChild('frmLogin') frmLogin!: DxFormComponent;

  public returnUrl: string;
  public authenticateRequestFormData: AuthenticateRequest;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private notifierService: NotifierService,
    private spinnerService: NgxSpinnerService,
    private languageService: LanguageService,
    private authenticationService: AuthenticationService) {

    this.authenticate_onClick = this.authenticate_onClick.bind(this);

    this.returnUrl = '';
    this.authenticateRequestFormData = { username: '', password: '', }

    if (this.authenticationService.userValue) {
      this.router.navigate(['/']);
    }

  }

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  public authenticate_onClick(): void {
    const validate = this.frmLogin.instance.validate();

    if (!validate.isValid) {
      return;
    }

    this.spinnerService.show();

    this.authenticationService
      .login(this.authenticateRequestFormData.username, this.authenticateRequestFormData.password)
      .pipe(first())
      .subscribe({
        next: () => {
          this.router.navigate([this.returnUrl]);
          this.spinnerService.hide();
        },
        error: error => {
          this.notifierService.notify('error', error);
          this.spinnerService.hide();
        }
      });
  }
}

