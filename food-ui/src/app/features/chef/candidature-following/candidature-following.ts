import { Component, computed, inject, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ChefPortalService } from '../../../core/services/chef-portal.service';
import { ChefApplication } from '../../../shared/models/chef.models';

@Component({ selector:'app-candidature-following', standalone:true, imports:[DatePipe,DecimalPipe,FormsModule,RouterLink], templateUrl:'./candidature-following.html', styleUrl:'./candidature-following.css' })
export class CandidatureFollowingComponent {
  readonly data = inject(ChefPortalService);
  readonly query = signal(''); readonly statusFilter = signal('all'); readonly pageSize = signal(5); readonly currentPage = signal(1);
  readonly sortKey = signal<keyof ChefApplication>('date'); readonly sortDirection = signal(-1);
  readonly pendingCount = computed(() => this.data.applications().filter(item => item.status === 'pending').length);
  readonly acceptedCount = computed(() => this.data.applications().filter(item => item.status === 'accepted').length);
  readonly filtered = computed(() => {
    const query=this.query().trim().toLowerCase(); const status=this.statusFilter(); const key=this.sortKey(); const direction=this.sortDirection();
    return this.data.applications().filter(item => (status==='all'||item.status===status) && (!query||`${item.id} ${item.restaurantName} ${item.address} ${item.status}`.toLowerCase().includes(query)))
      .sort((a,b) => String(a[key]).localeCompare(String(b[key]),undefined,{numeric:true})*direction);
  });
  readonly pageCount = computed(() => Math.max(1,Math.ceil(this.filtered().length/this.pageSize())));
  readonly pages = computed(() => Array.from({length:this.pageCount()},(_,i)=>i+1));
  readonly visibleItems = computed(() => { const page=Math.min(this.currentPage(),this.pageCount()); const start=(page-1)*this.pageSize(); return this.filtered().slice(start,start+this.pageSize()); });
  constructor() { void this.data.loadApplications(); }
  sortBy(key:keyof ChefApplication): void { if(this.sortKey()===key)this.sortDirection.update(value=>-value);else{this.sortKey.set(key);this.sortDirection.set(1);} }
  setPage(page:number): void { this.currentPage.set(Math.max(1,Math.min(page,this.pageCount()))); }
  rangeStart(): number { return this.filtered().length ? (this.currentPage()-1)*this.pageSize()+1 : 0; }
  rangeEnd(): number { return Math.min(this.currentPage()*this.pageSize(),this.filtered().length); }
  statusLabel(status:string): string { return status==='pending'?'In review':status==='accepted'?'Accepted':'Not selected'; }
}
