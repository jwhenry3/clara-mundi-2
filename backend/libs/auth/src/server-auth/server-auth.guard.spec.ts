import { ServerAuthGuard } from './server-auth.guard';

describe('ServerAuthGuard', () => {
  it('should be defined', () => {
    expect(new ServerAuthGuard()).toBeDefined();
  });
});
