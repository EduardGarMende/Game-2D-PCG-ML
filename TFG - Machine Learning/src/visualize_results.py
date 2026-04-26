import pandas as pd
import numpy as np
import os
import matplotlib.pyplot as plt
import seaborn as sns

from sklearn.compose import ColumnTransformer
from sklearn.preprocessing import StandardScaler, OneHotEncoder
from sklearn.ensemble import RandomForestClassifier
from sklearn.model_selection import cross_val_predict
from sklearn.metrics import confusion_matrix

def main():
    print("="*50)
    print(" GENERANDO GRÁFICOS DE RESULTADOS ")
    print("="*50)

    graphs_dir = 'graphs'
    os.makedirs(graphs_dir, exist_ok=True)

    # 2. Cargar datos
    data_path = os.path.join('data', 'telemetria_entrenamiento.csv')
    df = pd.read_csv(data_path)
    X = df.drop(columns=['PerfilJugador'])
    y = df['PerfilJugador']

    # 3. Configurar Preprocesador y Modelo
    numeric_features = ['APM', 'Precision', 'AvgDistance', 'DamageTakenPerMin', 
                        'DamageDealtPerMin', 'RangedRatio', 'DashPerMin', 
                        'ShieldPerMin', 'TimeInRiskZone']
    categorical_features = ['RewardChosen']

    preprocessor = ColumnTransformer(
        transformers=[
            ('num', StandardScaler(), numeric_features),
            ('cat', OneHotEncoder(handle_unknown='ignore'), categorical_features)
        ])

    rf_model = RandomForestClassifier(n_estimators=100, random_state=42, class_weight='balanced')

    # ==========================================
    # ANÁLISIS 1: MATRIZ DE CONFUSIÓN
    # ==========================================
    print("-> Generando Matriz de Confusión...")
    
    # Hacemos predicciones usando validación cruzada para que sea realista
    from sklearn.pipeline import Pipeline
    pipeline = Pipeline(steps=[('preprocessor', preprocessor), ('classifier', rf_model)])
    y_pred = cross_val_predict(pipeline, X, y, cv=5)

    # Nombres únicos de las clases (perfiles)
    classes = np.unique(y)
    cm = confusion_matrix(y, y_pred, labels=classes)

    plt.figure(figsize=(8, 6))
    sns.heatmap(cm, annot=True, fmt='d', cmap='Blues', xticklabels=classes, yticklabels=classes)
    plt.title('Matriz de Confusión (Random Forest CV=5)')
    plt.ylabel('Perfil Real')
    plt.xlabel('Perfil Predicho por modelo')
    plt.tight_layout()
    
    cm_path = os.path.join(graphs_dir, 'confusion_matrix.png')
    plt.savefig(cm_path, dpi=300)
    plt.close()
    print(f"   [Guardado] {cm_path}")

    # ==========================================
    # ANÁLISIS 2: FEATURE IMPORTANCE
    # ==========================================
    print("-> Extrayendo la Importancia de las Características...")

    # Entrenamos el pipeline con TODOS los datos para extraer la importancia final
    pipeline.fit(X, y)

    # Obtener el modelo y el preprocesador ya entrenados desde el pipeline
    trained_rf = pipeline.named_steps['classifier']
    trained_prep = pipeline.named_steps['preprocessor']

    # Extraer los nombres de las variables después del preprocesamiento
    cat_encoder = trained_prep.named_transformers_['cat']
    cat_features_encoded = cat_encoder.get_feature_names_out(categorical_features)
    all_feature_names = numeric_features + list(cat_features_encoded)

    # Extraer la importancia porcentual
    importances = trained_rf.feature_importances_

    # Crear un DataFrame para ordenarlas
    feature_importance_df = pd.DataFrame({
        'Feature': all_feature_names,
        'Importance': importances
    }).sort_values(by='Importance', ascending=False)

    # Crear gráfico de barras horizontales
    plt.figure(figsize=(10, 8))
    sns.barplot(x='Importance', y='Feature', data=feature_importance_df, palette='viridis')
    plt.title('Importancia de las Variables para el Modelo (Feature Importance)')
    plt.xlabel('Peso en la decisión')
    plt.ylabel('Métrica Telemetría')
    plt.tight_layout()

    fi_path = os.path.join(graphs_dir, 'feature_importance.png')
    plt.savefig(fi_path, dpi=300)
    plt.close()
    print(f"   [Guardado] {fi_path}")

    print("="*50)
    print("¡Gráficos generados con éxito! Revisa la carpeta 'graphs/'.")

if __name__ == "__main__":
    main()