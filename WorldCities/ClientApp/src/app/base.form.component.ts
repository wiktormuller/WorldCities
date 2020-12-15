import { Component } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  template: ''
})

export class BaseFormComponent
{
  form: FormGroup;  //The form model

  constructor() { }

  //Retrieve a FormControl
  getControl(name: string)
  {
    return this.form.get(name);
  }

  //Returns True if the FormControl is valid
  isValid(name: string)
  {
    var e = this.getControl(name);
    return e && e.valid;
  }

  //Returns True if the FormControl has been changed
  isChanged(name: string)
  {
    var e = this.getControl(name);
    return e && (e.dirty || e.touched);
  }

  //Return True if the FormControl is raising an error, i.e. an invalid state after user changes
  hasError(name: string)
  {
    var e = this.getControl(name);
    return e && (e.dirty || e.touched) && e.invalid;
  }
}
