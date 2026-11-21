export interface Order {
  id: string;
  customer: string;
  initials: string;
  items: string;
  total: number;
  status: 'Preparing' | 'On the way' | 'Delivered' | 'Cancelled';
  time: string;
}
