import { Permission } from './permission.model';
// role mdoel
export class Role {
  constructor(
    public name = '',
    public description = '',
    public permissions: Permission[] = []
  ) { }

  public usersCount = 0;
  public id = '';
}
