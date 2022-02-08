import { Injectable } from '@angular/core'
import { formatMessage } from 'devextreme/localization'

import { Language } from '../models/language.model'

@Injectable({
  providedIn: 'root'
})
export class LanguageService {

  constructor() { }

  public list(): Array<Language> {
    return [
      { name: 'Español', value: 'es' },
      { name: 'English', value: 'en' },
      { name: 'Italiano', value: 'it' }
    ];
  }

  public getWord(key: string): string {
    return formatMessage(key);
  }
}
