import { Injectable } from '@angular/core';
import { CanActivate, CanActivateChild, CanDeactivate, CanLoad, Router } from '@angular/router';
import { EquipmentsComponent } from '../../Components/Equipment/Equipment.component';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate, CanActivateChild,CanDeactivate<EquipmentsComponent>, CanLoad  {
  constructor(private router: Router) {}

  canActivate(): boolean {
    return this.checkAuth();
  }

  canActivateChild(): boolean {
    return this.checkAuth();
  }

  canDeactivate(component: EquipmentsComponent): boolean {
    return true;
  }

  canLoad(): boolean {
    return this.checkAuth();
  }

  private checkAuth(): boolean {
      return true;
  }

}