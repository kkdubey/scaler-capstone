// user model
export class User {
  constructor(
    public id = '',
    public userName = '',
    public fullName = '',
    public email = '',
    public jobTitle = '',
    public phoneNumber = '',
    roles: string[] = []
  ) {
    if (roles)
      this.roles = roles;
  }

  public isEnabled = true;
  public isLockedOut = false;
  public roles: string[] = [];
  get friendlyName() {
    const name = this.fullName || this.userName;
    return this.jobTitle ? this.jobTitle + ' ' + name : this.jobTitle;
  }

}
