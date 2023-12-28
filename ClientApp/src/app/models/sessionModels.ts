import { SessionState } from './enums';

export interface SessionEntity {
  sessionGuidId: string;
  sessionNodeId: string;
  name: string;
  endpointUrl: string;
  state: SessionState;
  //
  isSelected: boolean;
}
