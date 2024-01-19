import { SessionState } from './enums';

export interface SessionEntity {
  guid: string;
  sessionNodeId: string;
  name: string;
  endpointUrl: string;
  state: SessionState;
  //
  isSelected: boolean;
}
