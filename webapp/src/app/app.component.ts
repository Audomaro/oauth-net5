import { Component } from '@angular/core'
import { Router } from '@angular/router'

import { AuthenticationService } from './services'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  public title = 'webapp';
  public isDrawerOpen: boolean;
  public navigation: { id: number, text: string, icon: string, path: string }[];
  public toolbarContent: any[];
  public isAuthenticated: boolean;

  constructor(
    private router: Router,
    private authenticationService: AuthenticationService
  ) {

    this.isAuthenticated = false;
    this.isDrawerOpen = false;
    this.toolbarContent = [{
      widget: 'dxButton',
      location: 'before',
      options: {
        icon: 'menu',
        onClick: () => this.isDrawerOpen = !this.isDrawerOpen,
      },
    }];

    this.navigation = [
      { id: 1, text: 'Home', icon: 'home', path: 'home' },
      { id: 2, text: 'Usuarios', icon: 'group', path: 'users' },
      { id: 3, text: 'Productos', icon: 'product', path: 'products' },
      { id: 4, text: 'Ventas', icon: 'money', path: 'sales' },
      { id: 5, text: 'Reportes', icon: 'chart', path: 'reports' },
      { id: 6, text: 'Salir', icon: 'runner', path: 'logout' },
    ];

  }

  ngOnInit(): void {
    this.authenticationService.user.subscribe(user => {
      this.isAuthenticated = user.jwtToken !== undefined;
    });

  }
  public menu_onItemClick(e: any): void {
    this.isDrawerOpen = false;

    if (e.itemData.path === 'logout') {
      this.authenticationService.logout();
    }

    this.router.navigate([e.itemData.path]);
  }
}
