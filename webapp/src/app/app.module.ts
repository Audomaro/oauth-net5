import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http'
import { APP_INITIALIZER, CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core'
import { BrowserModule } from '@angular/platform-browser'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'
import { NotifierModule } from 'angular-notifier'
import { DxButtonModule, DxDataGridModule, DxDrawerModule, DxFormModule, DxListModule, DxToolbarModule } from 'devextreme-angular'
import config from 'devextreme/core/config'
import { loadMessages, locale } from 'devextreme/localization'
import esMessages from 'devextreme/localization/messages/es.json'
import itMessages from 'devextreme/localization/messages/it.json'
import { NgxSpinnerModule } from 'ngx-spinner'

import { AppRoutingModule } from './app-routing.module'
import { AppComponent } from './app.component'
import { appInitializer } from './helpers/app.initializer'
import { ErrorInterceptor, JwtInterceptor } from './interceptors'
import { HomeComponent } from './pages/home/home.component'
import { LoginComponent } from './pages/login/login.component'
import { ProductsComponent } from './pages/products/products.component'
import { ReportsComponent } from './pages/reports/reports.component'
import { SalesComponent } from './pages/sales/sales.component'
import { UsersComponent } from './pages/users/users.component'
import { AuthenticationService } from './services'

config({
  editorStylingMode: 'underlined',
});

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    LoginComponent,
    UsersComponent,
    ReportsComponent,
    SalesComponent,
    ProductsComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    HttpClientModule,
    // Devextreme
    DxDrawerModule,
    DxToolbarModule,
    DxListModule,
    DxFormModule,
    DxButtonModule,
    DxDataGridModule,
    // 3rd party
    NotifierModule.withConfig({
      behaviour: {
        stacking: 5
      },
      position: {
        horizontal: {
          position: 'right'
        },
        vertical: {
          position: 'bottom'
        }
      },

    }),
    NgxSpinnerModule,
  ],
  providers: [
    { provide: APP_INITIALIZER, useFactory: appInitializer, multi: true, deps: [AuthenticationService] },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule {
  constructor() {
    loadMessages(esMessages);
    loadMessages(itMessages);

    locale('es');

    loadMessages({
      'en': {
        'language': 'Language',
        'btn-text-login': 'Login'
      },
      'es': {
        'language': 'Lenguaje',
        'btn-text-login': 'Iniciar sesión'
      },
      'it': {
        'language': 'Linguaggio',
        'btn-text-login': 'Login'
      }
    });
  }
}
