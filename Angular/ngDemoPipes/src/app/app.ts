import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TempConverterPipe } from './temp-converter-pipe';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule, TempConverterPipe, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('Pipes Demo');
  toDate: Date = new Date();
  message: string ="Welcome to Angular!";
  num: number = 9542.125682;
  per: number = .74142;
  curr: number = 285;

  obj: object = {
    a: 789,
    b: 456,
    c: 123
  };

  mapobj: Map<string, number> =new Map([
    ['a', 123],
    ['b', 456],
    ['c', 789]
  ]);

  Farenheit: number=0;
  Celsius: number=0;
}