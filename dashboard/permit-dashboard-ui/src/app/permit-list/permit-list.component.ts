import { Component, OnInit } from '@angular/core';
import { ApiService, PermitRequestMessage } from '../core/api.service';

@Component({
  selector: 'app-permit-list',
  templateUrl: './permit-list.component.html'
})
export class PermitListComponent implements OnInit {
  permits: PermitRequestMessage[] = [];

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.apiService.getPermitList().subscribe((data) => (this.permits = data));
  }
}
