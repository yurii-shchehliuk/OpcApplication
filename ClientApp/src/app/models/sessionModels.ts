export interface SessionEntity {
  id: number;
  sessionId: string;
  sessionNodeId: string;
  name: string;
  endpointUrl: string;
  state: SessionState;
  nodeConfigs: null;
}

export enum SessionState {
  connected,
  disconnected,
  connecting,
}
