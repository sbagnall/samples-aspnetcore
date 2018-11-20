import { OktaAuthService } from '@okta/okta-angular';

export class AdminService {
    public constructor(private oktaAuth: OktaAuthService) {

    }

    public async isInAdminGroup(): Promise<boolean> {
        const claims = await this.oktaAuth.getUser();
        return claims.hasOwnProperty('admin_group');
    }
}
