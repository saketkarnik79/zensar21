import { Component, signal } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
//import { HomeComponent } from "./home-component/home-component";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink],//, HomeComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('Angular Routing Demo');
}
