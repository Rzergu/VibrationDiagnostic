import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SensorAnaliticComponent } from './sensor-analitic.component';

describe('SensorAnaliticComponent', () => {
  let component: SensorAnaliticComponent;
  let fixture: ComponentFixture<SensorAnaliticComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SensorAnaliticComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SensorAnaliticComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
