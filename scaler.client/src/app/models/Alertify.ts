export declare let alertify: Alertify;

export interface Alertify {
  log(message: string, type?: string, wait?: number): Alertify;

  prompt(message: string, callback?: (ok: boolean, val: string) => void, placeholder?: string, cssClass?: string): Alertify;

  success(message: string): Alertify;

  error(message: string): Alertify;

  set(args: Properties): void;
  
  alert(message: string, callback?: (ok: boolean) => void, cssClass?: string): Alertify;

  confirm(message: string, callback?: (ok: boolean) => void, cssClass?: string): Alertify;

  extend(type: string): (message: string, wait?: number) => Alertify;

  init(): void;

  labels: Labels;

  debug(): void;
}

export interface Properties {
  buttonFocus?: string | undefined;
  buttonReverse?: boolean | undefined;
  delay?: number | undefined;
  labels?: Labels | undefined;
}

export interface Labels {
  cancel?: string | undefined;
  ok?: string | undefined;
}

