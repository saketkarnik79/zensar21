import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('ngDemoTemplateDrivenFormsValidation');

  user = {
    name: '',
    email: '',
    password: '',
    age: null,
    country: '',
    terms: false
  }; 

  submitted: boolean = false;

  countries:string[] = ["India", "USA", "UK", "Canada"];

  onSubmit(form: NgForm){
    this.submitted = true;
    if(form.valid){
      console.log(`Form submitted: ${this.user}`);
    }
    else {
      console.log("Form is Invalid");
    }
  }

  resetForm(form: NgForm){
    form.reset();
    this.submitted = false;
  }
}
