import { Component, input } from '@angular/core';
import { DashboardStat } from '../../models/dashboard.model';
import { IconComponent } from '../icon/icon';

@Component({
  selector: 'app-stat-card',
  standalone: true,
  imports: [IconComponent],
  templateUrl: './stat-card.html',
  styleUrl: './stat-card.css',
})
export class StatCardComponent {
  stat = input.required<DashboardStat>();
}
