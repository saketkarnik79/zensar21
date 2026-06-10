import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Child1 } from './child1/child1';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Child1],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('ng-DemoComponents');
}
