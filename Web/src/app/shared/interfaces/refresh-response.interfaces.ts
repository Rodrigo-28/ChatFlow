export interface RefreshResponse {
  token: string;         // nuevo JWT
  refreshToken?: string; // algunos backends rotan el refresh; si viene, hay que guardarlo
}
