import joblib
import pandas as pd
import os

def main():
    model_path = os.path.join('models', 'svm_player_profile_model.joblib')
    
    if not os.path.exists(model_path):
        print(f"Error: No se encuentra el modelo en {model_path}")
        return

    print("Cargando el cerebro de la IA...")
    pipeline = joblib.load(model_path)

    # Simular a un jugador Tactico:
    nuevo_jugador = {
        'APM': [18.32],
        'Precision': [0.96],
        'AvgDistance': [6.47],
        'DamageTakenPerMin': [16.88],
        'DamageDealtPerMin': [624.48],
        'RangedRatio': [1.00],
        'DashPerMin': [6.19],
        'ShieldPerMin': [1.60],
        'TimeInRiskZone': [0.09],
        'RewardChosen': ['Bow_Damage']
    }

    df_nuevo = pd.DataFrame(nuevo_jugador)

    prediccion = pipeline.predict(df_nuevo)
    
    print("\n" + "="*40)
    print(" ANÁLISIS DE LA NUEVA SALA ")
    print("="*40)
    print(df_nuevo.iloc[0].to_string())
    print("-" * 40)
    print(f"PREDICCIÓN DEL MODELO: Jugador clasificado como [{prediccion[0]}]!")
    print("="*40)

if __name__ == "__main__":
    main()