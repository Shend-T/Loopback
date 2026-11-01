import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IssueModal } from './issue-modal';

describe('IssueModal', () => {
  let component: IssueModal;
  let fixture: ComponentFixture<IssueModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [IssueModal],
    }).compileComponents();

    fixture = TestBed.createComponent(IssueModal);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
