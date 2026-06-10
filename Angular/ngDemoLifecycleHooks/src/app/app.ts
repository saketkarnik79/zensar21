import { Component, signal, OnInit, OnChanges, DoCheck, AfterContentInit, 
  AfterContentChecked, AfterViewInit, AfterViewChecked, OnDestroy, 
  SimpleChanges} from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit, OnChanges, DoCheck, AfterContentInit, AfterContentChecked,
  AfterViewInit, AfterViewChecked, OnDestroy {
  protected readonly title = signal('ngDemoLifecycleHooks');

  ngOnInit(): void {
    console.log('ngOnInit() Invoked...');
  }

  ngOnChanges(changes: SimpleChanges): void {
    console.log('ngOnChanges() Invoked...');
  }

  ngDoCheck(): void {
    console.log('ngDoCheck() Invoked...');
  }

  ngAfterContentInit(): void {
    console.log('ngAfterContentInit() Invoked...');
  }

  ngAfterContentChecked(): void {
    console.log('ngAfterContentChecked() Invoked...');
  }

  ngAfterViewInit(): void {
    console.log('ngAfterViewInit() Invoked...');
  }

  ngAfterViewChecked(): void {
    console.log('ngAfterViewChecked() Invoked...');
  }

  ngOnDestroy(): void {
    console.log('ngOnDestroy() Invoked...');
  }
}
