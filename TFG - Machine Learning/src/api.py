from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import pandas as pd
import joblib
import uvicorn
import os

app = FastAPI(title="API telemetria TFG", description="Clasificador SVM de perfiles de jugador en tiempo real.")

model_pipeline = None

@app.on_event("startup")
def load_model():
    global model_pipeline
    model_path = os.path.join("models", 'svm_player_profile_model.joblib')
    if not os.path.exists(model_path):
        raise FileNotFoundError(f"Modelo no encontrado en {model_path}")
    model_pipeline = joblib.load(model_path)

class PlayerMetrics(BaseModel):
    APM: float
    Precision: float
    AvgDistance: float
    DamageTakenPerMin: float
    DamageDealtPerMin: float
    RangedRatio: float
    DashPerMin: float
    ShieldPerMin: float
    TimeInRiskZone: float
    RewardChosen: str

@app.post("/predict")
def predict_profile(metrics: PlayerMetrics):
    if model_pipeline is None:
        raise HTTPException(status_code=500, detail="Modelo no cargado")
    
    data_dict = metrics.model_dump()
    df = pd.DataFrame([data_dict])

    try:
        prediction = model_pipeline.predict(df)
        return {"predicted_profile": str(prediction[0])}
    except Exception as e:
        raise HTTPException(status_code=400, detail=f"Error en la predicción: {str(e)}")

if __name__ == "__main__":
    uvicorn.run("api:app", host="127.0.0.1", port=8000, reload=True)