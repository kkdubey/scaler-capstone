import { PermissionValues } from './permission.model';
// login response
export interface LoginResponse {
  access_token: string;
  token_type: string;
  scope: string;
  id_token: string;
  refresh_token: string;
  expires_in: number;
}
// idtoken model
export interface IdToken {
  permission: PermissionValues | PermissionValues[];
  name: string;
  fullname: string;
  jobtitle: string;
  email: string;
  phone_number: string;
  configuration: string;
  aud: string | string[];
  sub: string;
  role: string | string[];
  iat: number;
  exp: number;
  iss: string;
}
