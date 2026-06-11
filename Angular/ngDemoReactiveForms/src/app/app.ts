import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { Country } from './models/country';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule, ReactiveFormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('ngDemoReactiveForms');

  contactForm:FormGroup = new FormGroup({
    firstname: new FormControl('', [Validators.required, Validators.minLength(2)]),
    lastname: new FormControl('', [Validators.required]),
    email: new FormControl(''),
    gender: new FormControl('', [Validators.required]),
    ismarried: new FormControl('',[Validators.required]),
    country: new FormControl('', [Validators.required]),
  });

  countryList: Country[] = [
    {id: '1', name: 'India'},
    {id: '2', name: 'USA'},
    {id: '3', name: 'UK'},
  ];

  onSubmit(){
    console.log(this.contactForm.value);
  }
}
