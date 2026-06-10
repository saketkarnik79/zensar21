import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Movie } from './models/movie';
import { CommonModule } from '@angular/common';
import { Item } from './models/item';
import { FormsModule } from '@angular/forms';
import { TtClass } from './directives/tt-class';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule, FormsModule, TtClass],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  //protected readonly title = signal('Top 5 Movies');

  // movies: Movie[] = [
  //   {title:'Zootopia',director:'Byron Howard, Rich Moore',cast:'Idris Elba, Ginnifer Goodwin, Jason Bateman',releaseDate:'March 4, 2016'},
  //   {title:'Batman v Superman: Dawn of Justice',director:'Zack Snyder',cast:'Ben Affleck, Henry Cavill, Amy Adams',releaseDate:'March 25, 2016'},
  //   {title:'Captain American: Civil War',director:'Anthony Russo, Joe Russo',cast:'Scarlett Johansson, Elizabeth Olsen, Chris Evans',releaseDate:'May 6, 2016'},
  //   {title:'X-Men: Apocalypse',director:'Bryan Singer',cast:'Jennifer Lawrence, Olivia Munn, Oscar Isaac',releaseDate:'May 27, 2016'},
  //   {title:'Warcraft',director:'Duncan Jones',cast:'Travis Fimmel, Robert Kazinsky, Ben Foster',releaseDate:'June 10, 2016'}
  // ];

  // protected readonly title = signal('ngSwitch Demo');

  // items: Item[] = [
  //   {name: 'One', val: 1}, 
  //   {name: 'Two', val: 2}, 
  //   {name: 'Three', val: 3}, 
  //   {name: 'Four', val: 3}, 
  //   {name: 'Five', val: 3}
  // ];
  // selectedValue1: string = 'One';

  // protected readonly title = signal('ngIf Demo');

  // showMe: boolean = false;

  // cssVar: string = 'primary italics is-active';
  // cssArray = ['secondary', 'is-active', 'big'];
  // cssClass = {
  //   primary: true,
  //   big: true
  // }


}
