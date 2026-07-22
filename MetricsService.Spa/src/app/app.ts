import { Component } from '@angular/core';
import { Upload } from './sections/upload';
import { Results } from './sections/results';
import { Latest } from './sections/latest';

@Component({
  selector: 'app-root',
  imports: [Upload, Results, Latest],
  templateUrl: './app.html',
})
export class App {}
