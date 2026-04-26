import pandas as pd
import numpy as np
import os
import joblib

from sklearn.compose import ColumnTransformer
from sklearn.preprocessing import OneHotEncoder, StandardScaler
from sklearn.pipeline import Pipeline

from sklearn.ensemble import RandomForestClassifier
from sklearn.svm import SVC
from sklearn.neighbors import KNeighborsClassifier

from sklearn.model_selection import cross_validate, StratifiedKFold
import warnings

warnings.filterwarnings("ignore")

def main():
    print("="*50)
    print(" INICIANDO PIPELINE DE MACHINE LEARNING ")
    print("="*50)

    # 1. CARGA DE DATOS
    data_path = os.path.join('data', 'telemetria_entrenamiento.csv')
    
    if not os.path.exists(data_path):
        print(f"ERROR: No se encontró el dataset en {data_path}")
        return

    print("-> Cargando dataset...")
    df = pd.read_csv(data_path)
    print(f"Muestras cargadas: {df.shape[0]} | Características: {df.shape[1] - 1}")

    X = df.drop(columns=['PerfilJugador'])
    y = df['PerfilJugador']

    # 2. DEFINICIÓN DEL PREPROCESAMIENTO
    print("-> Configurando el preprocesador (StandardScaler + OneHotEncoder)...")
    
    numeric_features = [
        'APM', 'Precision', 'AvgDistance', 'DamageTakenPerMin', 
        'DamageDealtPerMin', 'RangedRatio', 'DashPerMin', 
        'ShieldPerMin', 'TimeInRiskZone'
    ]
    categorical_features = ['RewardChosen']

    preprocessor = ColumnTransformer(
        transformers=[
            ('num', StandardScaler(), numeric_features),
            ('cat', OneHotEncoder(handle_unknown='ignore'), categorical_features)
        ])

    # 3. CONFIGURACIÓN DE LOS MODELOS
    # class_weight='balanced' para compensar que 'Expert' tiene más muestras
    models = {
        'Random Forest': RandomForestClassifier(n_estimators=100, random_state=42, class_weight='balanced'),
        'SVM': SVC(kernel='rbf', random_state=42, class_weight='balanced'),
        'KNN': KNeighborsClassifier(n_neighbors=5) 
    }

    # 4. VALIDACIÓN CRUZADA (K-Fold K=5)
    print("-> Evaluando modelos con Stratified 5-Fold Cross-Validation...\n")
    
    cv_strategy = StratifiedKFold(n_splits=5, shuffle=True, random_state=42)
    
    scoring_metrics = ['accuracy', 'precision_macro', 'recall_macro', 'f1_macro']

    best_model_name = "SVM"  # Basado en pruebas previas, SVM es el mejor modelo para este dataset

    for name, model in models.items():
        # Construir el pipeline completo: Preprocesamiento + Modelo
        pipeline = Pipeline(steps=[('preprocessor', preprocessor),
                                   ('classifier', model)])
        
        # Ejecutar Validación Cruzada
        cv_results = cross_validate(pipeline, X, y, cv=cv_strategy, scoring=scoring_metrics)
        
        # Calcular medias de los 5 folds
        acc_mean = np.mean(cv_results['test_accuracy'])
        prec_mean = np.mean(cv_results['test_precision_macro'])
        rec_mean = np.mean(cv_results['test_recall_macro'])
        f1_mean = np.mean(cv_results['test_f1_macro'])
        
        # Imprimir resultados
        print(f"[{name}]")
        print(f"  Accuracy : {acc_mean:.4f}")
        print(f"  Precision: {prec_mean:.4f} (Macro)")
        print(f"  Recall   : {rec_mean:.4f} (Macro)")
        print(f"  F1-Score : {f1_mean:.4f} (Macro)\n")

    # 5. ENTRENAMIENTO FINAL Y EXPORTACIÓN
    print("="*50)
    print(f"-> Entrenando el modelo final ({best_model_name}) con el 100% de los datos...")
    
    final_pipeline = Pipeline(steps=[('preprocessor', preprocessor),
                                     ('classifier', models[best_model_name])])
    
    # Entrenar con todo el dataset
    final_pipeline.fit(X, y)

    models_dir = 'models'
    os.makedirs(models_dir, exist_ok=True)
    
    # Exportar el pipeline completo (incluye el preprocesador)
    export_path = os.path.join(models_dir, 'svm_player_profile_model.joblib')
    joblib.dump(final_pipeline, export_path)
    
    print(f"¡Éxito! Pipeline final exportado a: {export_path}")
    print("="*50)

if __name__ == "__main__":
    main()