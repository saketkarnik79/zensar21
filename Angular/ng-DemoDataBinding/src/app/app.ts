import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('ng-DemoDataBinding');
  firstName: string;
  lastName: string;
  private count:number=0;

  isDisabled: boolean;

  data:string;

  constructor() {
    this.firstName = "James";
    this.lastName = "Bond";

    this.isDisabled = true;
    
    this.data="Initial Data";

    // setInterval(() => {
    //   this.count++;
    //   this.lastName = `Changed ${this.count}`;
    // }, 1000);
  }

  onSave(){
    console.log("Data Saved...");
  }

  OnChange(){
    console.log("Textbox data changed...");
  }

  OnChange2(event: Event): void {
    console.log(((event.target as HTMLInputElement) || { value: '' }).value);
  }

  ChangedData(){
    this.data="Modified Data";
  }
}
