import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { ApiService } from '../core/api.service';

@Component({
  selector: 'app-permit-status',
  templateUrl: './permit-status.component.html'
})
export class PermitStatusComponent implements OnChanges {
  @Input() applicationId?: number;
  statusMessage = 'Awaiting selection';

  constructor(private apiService: ApiService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['applicationId'] && this.applicationId) {
      this.apiService.getPermitStatus(this.applicationId).subscribe((status) => {
        this.statusMessage = status?.state ?? 'Pending update';
      });
    }
  }
}
