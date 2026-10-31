import { User } from './user';

export type IssueStatus = 'New' | 'Pending' | 'Resolved';
export type IssueSeverity = 'Low' | 'Normal' | 'High' | 'Critical';

export interface Issue {
  id: number;
  organizationId: number;
  title: string;
  description: string;
  status: IssueStatus;
  severity: IssueSeverity;
  reportedById: number | null;
  reportedBy: User | null;
  assignedToId: number | null;
  assignedTo: User | null;
  createdAt: string;
  dueDate: string | null;
}
