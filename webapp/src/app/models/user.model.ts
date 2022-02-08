
export interface User {
  id: number;
  username: string;
  branch: string;
  displayName: string;
  email: string;
  jwtToken?: string;
}
