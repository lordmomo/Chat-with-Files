import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DataSharingService {
  private summaryData = new BehaviorSubject<{ question: string, answer: string } | null>(null);
  currentSummaryData = this.summaryData.asObservable();

  updateSummaryData(data: { question: string, answer: string }) {
    this.summaryData.next(data);
  }
}
