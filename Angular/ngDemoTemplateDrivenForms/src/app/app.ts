import { Component, signal, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Country } from './models/country';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, FormsModule, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected readonly title = signal('Template Driven Forms Demo');
  countryList: Country[] = [
    {id: '1', name: 'India'},
    {id: '2', name: 'USA'},
    {id: '3', name: 'UK'},
  ];

  constructor(){

  }

  ngOnInit(): void {
    
  }

  onSubmit(contactForm:NgForm){
    console.log(contactForm.value);
  }
}
