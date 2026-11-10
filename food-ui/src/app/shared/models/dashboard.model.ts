export interface DashboardStat {
  label: string;
  value: string;
  change: string;
  icon: string;
  tone: 'mint' | 'peach' | 'lavender' | 'blue';
  points: string;
}

export interface Order {
  id: string;
  customer: string;
  initials: string;
  items: string;
  total: number;
  status: 'Preparing' | 'On the way' | 'Delivered' | 'Cancelled';
  time: string;
}

export interface NavigationItem {
  label: string;
  icon: string;
  route: string;
  badge?: string;
}
