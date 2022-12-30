import { Injectable } from '@nestjs/common';

@Injectable()
export class EquipmentServerService {
  getHello(): string {
    return 'Hello World!';
  }
}
