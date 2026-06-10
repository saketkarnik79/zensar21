import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CustomerComponent } from './customer-component/customer-component';
// import { OrderListComponent } from './order-list-component/order-list-component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CustomerComponent],//, OrderListComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('ngDemoParentChildCommunication');
}
