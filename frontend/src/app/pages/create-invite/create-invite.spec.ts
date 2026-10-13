import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateInvite } from './create-invite';

describe('CreateInvite', () => {
  let component: CreateInvite;
  let fixture: ComponentFixture<CreateInvite>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateInvite],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateInvite);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
