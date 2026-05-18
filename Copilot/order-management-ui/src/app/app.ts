import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { OrderListComponent } from './features/orders/order-list/order-list';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, OrderListComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('order-management-ui');
}
