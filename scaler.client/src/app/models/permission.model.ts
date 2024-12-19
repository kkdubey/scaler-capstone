export type PermissionNames =
  'View Roles' | 'Manage Roles' | 'Assign Roles' |
  'View Users' | 'Manage Users' ;

export type PermissionValues =
  'roles.view' | 'roles.manage' | 'roles.assign' |
  'users.view' | 'users.manage';

export interface Permission {
  name: PermissionNames;
  value: PermissionValues;
  description: string;
  groupName: string;
}

// permission constant
export class Permissions {
  public static readonly viewRoles: PermissionValues = 'roles.view';
  public static readonly manageRoles: PermissionValues = 'roles.manage';
  public static readonly assignRoles: PermissionValues = 'roles.assign';

  public static readonly viewUsers: PermissionValues = 'users.view';
  public static readonly manageUsers: PermissionValues = 'users.manage';
}
