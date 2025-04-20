import { SensorDataModel } from './SensorDataModel';
import { TimeValueModel } from './TimeValueModel';
export class Sensor{
    id: number;
    name: string;
    sensorValues: SensorDataModel[];
    averageValues: TimeValueModel[];
}