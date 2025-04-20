import { FrequencyModel } from './FrequencyModel';

export class SensorDataModel {
    SensorId: number;
    date: Date;
    SensorsFrequencyDataPoints: FrequencyModel[];
}