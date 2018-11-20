import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { OktaAuthGuard, OktaAuthService } from '@okta/okta-angular';
import { NgModule } from '@angular/core';
import { AdminService } from './adminService';

@NgModule()
export class AdminAuthGuard implements CanActivate {
    private oktaAuthGuard: OktaAuthGuard;
    private adminService: AdminService;

    constructor(private oktaAuth: OktaAuthService, router: Router) {
        this.oktaAuthGuard = new OktaAuthGuard(oktaAuth, router);
        this.adminService = new AdminService(oktaAuth);
    }
    async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
        const isCanActivate = await this.oktaAuthGuard.canActivate(route, state);
        const isAdmin = await this.adminService.isInAdminGroup();

        return isCanActivate && isAdmin;
    }
}
