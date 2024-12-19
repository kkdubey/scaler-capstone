// user login model
export class UserLogin {
  constructor(
    public userName = '',
    public password = '',
    public rememberMe?: boolean
  ) { }
}
