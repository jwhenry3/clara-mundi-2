import { Test, TestingModule } from '@nestjs/testing';
import { LoginServerController } from './login-server.controller';
import { LoginServerService } from './login-server.service';

describe('LoginServerController', () => {
  let loginServerController: LoginServerController;

  beforeEach(async () => {
    const app: TestingModule = await Test.createTestingModule({
      controllers: [LoginServerController],
      providers: [LoginServerService],
    }).compile();

    loginServerController = app.get<LoginServerController>(LoginServerController);
  });

  describe('root', () => {
    it('should return "Hello World!"', () => {
      expect(loginServerController.getHello()).toBe('Hello World!');
    });
  });
});
