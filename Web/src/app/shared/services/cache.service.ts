import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CacheService {
  private cache = new Map<string, any>();
  private subjects = new Map<string, BehaviorSubject<any>>();

  setItem(key: string, value: any): void {
    this.cache.set(key, value);
    if (this.subjects.has(key)) {
      this.subjects.get(key)!.next(value);
    } else {
      const subject = new BehaviorSubject<any>(value);
      this.subjects.set(key, subject);
    }
  }

  getItem<T>(key: string): Observable<T | null> {
    if (!this.subjects.has(key)) {
      const initialValue = this.cache.has(key) ? this.cache.get(key) : null;
      this.subjects.set(key, new BehaviorSubject<T | null>(initialValue));
    }
    return this.subjects.get(key)!.asObservable();
  }

  removeItem(key: string): void {
    this.cache.delete(key);
    if (this.subjects.has(key)) {
      this.subjects.get(key)!.next(null);
    }
  }

  clear(): void {
    this.cache.clear();
    this.subjects.forEach((subject) => subject.next(null));
  }
}
