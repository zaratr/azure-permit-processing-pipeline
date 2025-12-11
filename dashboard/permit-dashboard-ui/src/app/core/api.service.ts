import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, timer } from 'rxjs';
import { switchMap } from 'rxjs/operators';

export interface PermitRequestMessage {
  applicationId: number;
  applicantEmail: string;
  licenseType: string;
}

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly apiBaseUrl = 'http://localhost:5000/api';

  constructor(private http: HttpClient) {}

  enqueuePermit(request: PermitRequestMessage): Observable<any> {
    return this.http.post(`${this.apiBaseUrl}/queue/enqueue`, request);
  }

  getPermitList(): Observable<PermitRequestMessage[]> {
    return timer(0, 5000).pipe(
      switchMap(() => this.http.get<PermitRequestMessage[]>(`${this.apiBaseUrl}/permits`))
    );
  }

  getPermitStatus(applicationId: number): Observable<any> {
    return timer(0, 5000).pipe(
      switchMap(() => this.http.get(`${this.apiBaseUrl}/permits/${applicationId}/status`))
    );
  }
}
