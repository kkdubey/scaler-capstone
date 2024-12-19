import { Injectable } from '@angular/core';
import { AppTheme } from '../models/app-theme.model';


@Injectable()
export class ThemeManager {
  themes: Array<AppTheme> = [
    {
      id: 1,
      name: 'Default',
      href: 'bootstrap.css',
      isDefault: true,
      background: '#007bff',
      color: '#fff'
    },
    {
      id: 2,
      name: 'Cosmo',
      href: 'cosmo.css',
      background: '#2780E3',
      color: '#373a3c'
    },
    {
      id: 3,
      name: 'Lumen',
      href: 'lumen.css',
      background: '#158CBA',
      color: '#f0f0f0'
    }
  ];

  public installTheme(theme?: AppTheme) {
    if (theme == null || theme.isDefault) {
      this.removeStyle('theme');
    } else {
      this.setStyle('theme', theme.href);
    }
  }

  public getDefaultTheme(): AppTheme {
    const theme = this.themes.find(theme => theme.isDefault);

    if (!theme) {
      throw new Error('No default theme found!');
    }

    return theme;
  }

  public getThemeByID(id: number): AppTheme {
    const theme = this.themes.find(theme => theme.id === id);

    if (!theme)
      throw new Error(`Theme with id "${id}" not found!`);

    return theme;
  }

  private setStyle(key: string, href: string) {
    this.getLinkElementForKey(key).setAttribute('href', href);
  }

  private removeStyle(key: string) {
    const existingLinkElement = this.getExistingLinkElementByKey(key);
    if (existingLinkElement) {
      document.head.removeChild(existingLinkElement);
    }
  }

  private getLinkElementForKey(key: string) {
    return this.getExistingLinkElementByKey(key) || this.createLinkElementWithKey(key);
  }

  private getExistingLinkElementByKey(key: string) {
    return document.head.querySelector(`link[rel="stylesheet"].${this.getClassNameForKey(key)}`);
  }

  private createLinkElementWithKey(key: string) {
    const linkEl = document.createElement('link');
    linkEl.setAttribute('rel', 'stylesheet');
    linkEl.classList.add(this.getClassNameForKey(key));
    document.head.appendChild(linkEl);
    return linkEl;
  }

  private getClassNameForKey(key: string) {
    return `style-manager-${key}`;
  }
}
