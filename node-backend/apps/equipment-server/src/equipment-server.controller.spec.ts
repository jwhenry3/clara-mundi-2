import { Test, TestingModule } from '@nestjs/testing';
import { EquipmentServerController } from './equipment-server.controller';
import { EquipmentServerService } from './equipment-server.service';

describe('EquipmentServerController', () => {
  let equipmentServerController: EquipmentServerController;

  beforeEach(async () => {
    const app: TestingModule = await Test.createTestingModule({
      controllers: [EquipmentServerController],
      providers: [EquipmentServerService],
    }).compile();

    equipmentServerController = app.get<EquipmentServerController>(EquipmentServerController);
  });

  describe('root', () => {
    it('should return "Hello World!"', () => {
      expect(equipmentServerController.getHello()).toBe('Hello World!');
    });
  });
});
