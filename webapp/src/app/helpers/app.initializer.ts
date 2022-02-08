import { AuthenticationService } from '../services'

export function appInitializer(authenticationService: AuthenticationService): any {
  return () => new Promise((resolve: any) => {
    // attempt to refresh token on app start up to auto authenticate
    authenticationService.refreshToken()
      .subscribe()
      .add(resolve);
  });
}
