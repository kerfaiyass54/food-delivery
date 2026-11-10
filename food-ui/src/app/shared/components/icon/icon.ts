import { Component, input } from '@angular/core';

@Component({
  selector: 'app-icon',
  standalone: true,
  templateUrl: './icon.html',
})
export class IconComponent {
  name = input.required<string>();
  size = input(20);
}
